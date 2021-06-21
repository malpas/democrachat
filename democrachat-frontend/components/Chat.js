import { faChevronLeft } from "@fortawesome/free-solid-svg-icons"
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome"
import { useNavigate } from "@reach/router"
import { observer } from "mobx-react-lite"
import React, { useContext, useEffect, useRef, useState } from "react"
import { Helmet } from "react-helmet"
import GlobalContext from "../state"
import Authorized from "./Authorized"
import FinaliseModal from "./FinaliseModal"
import UserList from "./UserList"

const ChatMessages = ({ topic, messages, navigate }) => {
    const chatUserRef = useRef()

    useEffect(() => {
        chatUserRef.current.scrollTop = chatUserRef.current.scrollHeight
    }, [])

    return (
        <div className="chat__messages">
            <div className="chat__messages__header">
                <FontAwesomeIcon icon={faChevronLeft} size="lg" className="pointer" onClick={() => navigate("/topics")} />
                <h2>@{topic}</h2>
            </div>
            <div className="chat__messages__list" id="chat" ref={chatUserRef}>
                {messages.filter(message => message.topic === topic || message.topic === "all").map(message =>
                    <p className="chat__messages__message"><strong>{message.username}</strong> {message.text}</p>
                )}
            </div>
        </div>
    )
}

const ChatSender = ({ onSend, onChange }) => {
    const [message, setMessage] = useState("")

    const onLocalSubmit = ev => {
        onSend(message)
        setMessage("")
        ev.preventDefault()
    }
    const onLocalChange = ev => {
        setMessage(ev.target.value)
        onChange()
    }

    return (
        <div class="chat__send">
            <form onSubmit={onLocalSubmit} class="form form--chat">
                <input type="text" className="textbox" value={message} onChange={onLocalChange}></input>
                <input type="submit" class="button button--inline mt-0" value="Send"></input>
            </form>
        </div>
    )
}

const Chat = observer(({ topic }) => {
    const state = useContext(GlobalContext)
    const activeUsers = state.chat.activeUsers[topic]
    const navigate = useNavigate()

    const [isFinaliseOpen, setIsFinaliseOpen] = useState()

    var title = `Democrachat - ${topic}`
    if (state.chat.lastReceiveTime) {
        var timeSinceMessage = (new Date()).getTime() - state.chat.lastReceiveTime
        var increment = Math.floor(timeSinceMessage / 500)
        if (increment <= 4 && increment % 2 == 0) {
            title = `! DEMOCRACHAT - ${topic.toUpperCase()} !`
        }
    }


    useEffect(() => {
        state.chat.connect()
            .then(() => state.chat.joinTopic(topic))
            .then(() => state.chat.getActiveUsers(topic))

        state.peer.connectPeer()

        const handle = setInterval(() => {
            state.chat.joinTopic(topic)
        }, 1000)

        return () => {
            clearInterval(handle)
            state.chat.leaveTopic(topic)
        }
    }, [])


    const onSend = message => {
        state.chat.send(topic, message)
    }

    const onChange = () => {
        state.chat.indicateTyping(topic)
    }

    const finaliseUser = (username, password) => {
        state.auth.finalise(username, password).then(() => {
            setIsFinaliseOpen(false)
            state.peer.connectPeer()
            state.userActions.setSelectedUsername(null)
        })
    }

    return (
        <Authorized>
            <Helmet>
                <title>{title}</title>
            </Helmet>
            <div className="container chat">
                <FinaliseModal isOpen={isFinaliseOpen}
                    onClose={() => setIsFinaliseOpen(false)}
                    onSubmit={finaliseUser}
                    errors={state.auth.finaliseErrors} />
                <ChatMessages topic={topic} messages={state.chat.messages} navigate={navigate} />
                <ChatSender onChange={onChange} onSend={onSend} />
                <div class="chat__users">
                    <h2 class="chat__users__header mt-1">Users</h2>
                    <UserList usernames={activeUsers} typingIndicators={state.chat.typingIndicators} />
                    {state.auth.isGuest ? (
                        <strong class="action" onClick={() => setIsFinaliseOpen(true)}>Finalise your account</strong>
                    ) : null}
                </div>
            </div>
        </Authorized >
    )
})

export default Chat