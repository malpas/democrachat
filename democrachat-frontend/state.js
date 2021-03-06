import { HttpTransportType, HubConnectionBuilder, HubConnectionState, LogLevel } from "@microsoft/signalr"
import axios from "axios"
import { makeAutoObservable, runInAction } from "mobx"
import { createContext } from "react"

class RootStore {
    constructor() {
        this.auth = new AuthStore(this)
        this.chat = new ChatStore(this)
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
            .withUrl("/hub/chat", { transport: HttpTransportType.ServerSentEvents }) // TODO TRY REMOVING TRANSPORT
            .withAutomaticReconnect()
            .configureLogging(LogLevel.Debug)
            .build();

        this.connection.on("ReceiveMessage", (topic, username, text) => {
            runInAction(() => {
                this.messages = [...this.messages, { topic, username, text }]
            })
        })

        return this.connection.start()
    }

    getTopics() {
        axios.get("/api/topics", { withCredentials: true })
            .then(resp => {
                runInAction(() => {
                    this.topics = resp.data
                })
            })
    }

    joinTopic(name) {
        this.connection.send("JoinTopic", name)
    }

    send(topic, message) {
        this.connection.send("SendMessage", topic, message)
    }
}

class AuthStore {
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
        return axios.post("/api/auth/logout", null, { withCredentials: true })
    }

    fetchUserInfo() {
        return axios.get("/api/auth/info", { withCredentials: true })
            .then(resp => {
                runInAction(() => {
                    this.username = resp.data.username
                    this.isGuest = resp.data.isGuest
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
            .then(() => this.fetchUserInfo)
    }
}

const GlobalContext = createContext(new RootStore())

export default GlobalContext
export { RootStore }