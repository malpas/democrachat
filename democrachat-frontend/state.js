import { HttpTransportType, HubConnectionBuilder, HubConnectionState, LogLevel } from "@microsoft/signalr"
import axios from "axios"
import { makeAutoObservable, runInAction } from "mobx"
import { createContext } from "react"

class RootStore {
    constructor() {
        this.auth = new AuthStore(this)
        this.chat = new ChatStore(this)
        this.userActions = new UserActionStore(this)
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
        this.connection.onreconnected(() => this._setupConnection(this.connection))

        return this.connection.start()
    }

    _setupConnection(connection) {
        connection.on("ReceiveMessage", (topic, username, text) => {
            runInAction(() => {
                this.messages = [...this.messages, { topic, username, text }]
            })
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
            .catch(err => Promise.reject());
    }

    joinTopic(name) {
        return this.connection.send("JoinTopic", name)
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
    userActionErrors = []

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
            .then(() => {
                this.root.auth.fetchUserInfo()
            })
    }
}

const GlobalContext = createContext(new RootStore())

export default GlobalContext
export { RootStore }