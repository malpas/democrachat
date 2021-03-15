import { HttpTransportType, HubConnectionBuilder, HubConnectionState, LogLevel } from "@microsoft/signalr"
import axios from "axios"
import { makeAutoObservable, runInAction } from "mobx"
import Peer from "peerjs"
import { createContext, useContext } from "react"

import React from "react"
import { toast } from "react-toastify"

class RootStore {
    constructor() {
        this.auth = new AuthStore(this)
        this.chat = new ChatStore(this)
        this.userActions = new UserActionStore(this)
        this.peer = new PeerStore(this)
    }
}

class ChatStore {
    connection
    messages = []
    topics = []

    constructor(root) {
        this.root = root
        makeAutoObservable(this)
    }

    connect() {
        if (this.connection?.state == HubConnectionState.Connected) return Promise.resolve();

        this.connection = new HubConnectionBuilder()
            .withUrl("/hub/chat", { transport: HttpTransportType.ServerSentEvents })
            .withAutomaticReconnect()
            .configureLogging(LogLevel.Debug)
            .build();

        this._setupConnection(this.connection)
        this.connection.onreconnected(() => {
            toast("Reconnected to chat")
        })
        this.connection.onclose(() => {
            toast("Lost connection to chat")
        })

        return this.connection.start()
    }

    _setupConnection(connection) {
        window.apiMessages = () => this.messages
        connection.on("ReceiveMessage", (topic, username, text) => {
            runInAction(() => {
                this.messages = [...this.messages, { topic, username, text }]
            })
            var chat = document.querySelector("#chat")
            if (chat) {
                chat.scrollTop = chat.scrollHeight
            }
        })

    }

    disconnect() {
        if (this.connection?.state != HubConnectionState.Connected) return Promise.resolve();
        return this.connection.stop()
    }

    getTopics() {
        axios.get("/api/topics", { withCredentials: true })
            .then(resp => {
                runInAction(() => {
                    this.topics = resp.data
                })
            })
    }

    getActiveUsers(topic) {
        return axios.get(`/api/topicActivity/${topic}`, { withCredentials: true })
            .then(resp => {
                return Promise.resolve(resp.data)
            })
            .catch(_ => Promise.reject());
    }

    joinTopic(name) {
        if (this.connection?.state != HubConnectionState.Connected) return
        return this.connection.send("JoinTopic", name)
    }
    leaveTopic(name) {
        if (this.connection?.state != HubConnectionState.Connected) return
        return this.connection.send("LeaveTopic", name)
    }

    send(topic, message) {
        this.connection.send("SendMessage", topic, message)
    }
}

class AuthStore {
    gold
    silver
    username
    errorText
    finaliseErrors
    isGuest

    constructor(root) {
        this.root = root
        makeAutoObservable(this)
    }

    login(username, password) {
        return axios.post("/api/auth/login", { username, password }, { withCredentials: true })
            .catch(err => {
                if (err.response.status == 500 || err.response.status == 502) {
                    this.errorText = "Could not connect to server"
                    return Promise.reject()
                }
                this.errorText = err.response.data
                return Promise.reject(err)
            })
            .then(_ => {
                this.errorText = ""
                this.fetchUserInfo()
            })
    }

    register() {
        return axios.post("/api/auth/register", null, { withCredentials: true }).catch(err => {
            this.errorText = err.response.data
            return Promise.reject(err)
        }).then(_ => this.fetchUserInfo())
    }

    logout() {
        this.errorText = ""

        return axios.post("/api/auth/logout", null, { withCredentials: true }).then(() => {
            this.username = null
            this.root.chat.disconnect()
        })
    }

    fetchUserInfo() {
        return axios.get("/api/auth/info", { withCredentials: true })
            .then(resp => {
                runInAction(() => {
                    this.username = resp.data.username
                    this.isGuest = resp.data.isGuest
                    this.gold = resp.data.gold
                    this.silver = resp.data.silver
                })
            })
    }

    finalise(username, password) {
        return axios.post("/api/auth/finalize", { username, password }, { withCredentials: true })
            .catch(err => {
                if (err.response.data.errors) {
                    runInAction(() => {
                        this.finaliseErrors = Object.values(err.response.data.errors)
                    })
                }
                return Promise.reject()
            })
            .then(() => this.fetchUserInfo())
    }
}

class UserActionStore {
    bidErrors = []
    bidResult = ""
    userActionErrors = []
    userActionResult = ""
    selectedUsername

    constructor(root) {
        this.root = root
        makeAutoObservable(this)
    }

    muteUser(username, silver) {
        runInAction(() => {
            this.userActionErrors = []
        })
        if (!silver) {
            runInAction(() => {
                this.userActionErrors = ["Need amount of silver to mute"]
            })
            return
        }
        return axios.post("/api/mute", { username, silver }, { withCredentials: true })
            .catch(err => {
                runInAction(() => {
                    if (err.response.data.errors) {
                        this.userActionErrors = Object.values(err.response.data.errors)
                    } else {
                        this.userActionErrors = [err.response.data]
                    }
                })
            })
            .then(resp => {
                this.userActionResult = resp.data
                this.root.auth.fetchUserInfo()
                setTimeout(() => {
                    runInAction(() => {
                        this.userActionResult = ""
                    })
                }, 2000)
            })
    }

    topicBid(name, silver) {
        runInAction(() => {
            this.bidErrors = []
            this.bidResult = ""
        })
        return axios.post("/api/topic/bid", { name, silver }, { withCredentials: true })
            .catch(err => {
                runInAction(() => {
                    if (err.response.data.errors) {
                        this.bidErrors = Object.values(err.response.data.errors)
                    } else {
                        this.bidErrors = [err.response.data]
                    }
                })
                return Promise.reject()
            })
            .then(resp => {
                this.root.auth.fetchUserInfo()
                runInAction(() => {
                    this.bidResult = resp.data
                    setTimeout(() => {
                        this.bidResult = ""
                    }, 5000)
                })
            })
    }

    setSelectedUsername(username) {
        this.selectedUsername = username
    }
}

class PeerStore {
    peer
    isConnected = false
    currentCall
    localStream
    remoteStream

    constructor(root) {
        this.root = root
        makeAutoObservable(this)
    }

    connectPeer() {
        if (this.peer) this.peer.disconnect()

        this.peer = new Peer(null, { host: window.location.hostname, port: 9000 })
        this.peer.on('open', id => {
            axios.post(`/api/peer/${id}`, null, { withCredentials: true })
            this.isConnected = true
        })
        this.peer.on('call', call => {
            axios.get(`/api/peer/${call.peer}`, { withCredentials: true })
                .then(resp => {
                    const username = resp.data.username

                    const acceptCall = (closeToast) => {
                        this.root.userActions.setSelectedUsername(username)
                        navigator.mediaDevices.getUserMedia({ video: true, audio: true })
                            .then(stream => {
                                this.localStream = stream
                                call.on("stream", stream => this.remoteStream = stream)
                                call.on("close", () => this.currentCall = null)
                                call.answer(stream)
                                this.currentCall = call
                            })
                        closeToast()
                    }
                    const declineCall = (closeToast) => {
                        call.close()
                        closeToast()
                    }
                    toast(({ closeToast }) => <div>
                        <p>{username} wants to video chat.</p>
                        <p>
                            <button onClick={() => declineCall(closeToast)} className="button d-i">Decline</button>&nbsp;
                            <button onClick={() => acceptCall(closeToast)} className="button d-i">Accept</button>
                        </p>
                    </div>)
                })
        })
        this.peer.on('stream', stream => this.remoteStream = stream)
    }

    call(username) {
        if (!navigator.mediaDevices) {
            toast("No media devices")
            return
        }
        var hasMediaDevice = false
        navigator.mediaDevices.getUserMedia({ audio: true, video: true })
            .catch(_ => {
                toast("You'd need a camera and mic")
                return Promise.reject()
            })
            .then(stream => {
                hasMediaDevice = true
                this.localStream = stream
                return Promise.resolve()
            })
            .then(() => axios.get(`/api/peer/user/${username}`, { withCredentials: true }))
            .then(resp => {
                const id = resp.data.id
                var call = this.peer.call(id, this.localStream)
                call.on("stream", remoteStream => {
                    this.remoteStream = remoteStream
                })
                call.on("close", () => this.currentCall = null)
                this.currentCall = call
            })
            .catch(_ => {
                if (!hasMediaDevice)
                    return
                toast("Could not get data")
            })
    }

    endCall() {
        if (!this.currentCall) return
        if (this.localStream)
            this.localStream.getTracks().forEach(track => track.stop())
        if (this.remoteStream)
            this.remoteStream.getTracks().forEach(track => track.stop())
        this.currentCall.close()
        this.currentCall = null
    }
}

const GlobalContext = createContext(new RootStore())

export default GlobalContext
export { RootStore }