import { faChevronLeft } from "@fortawesome/free-solid-svg-icons"
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome"
import { useNavigate } from "@reach/router"
import { observer } from "mobx-react-lite"
import React, { useContext, useEffect, useState } from "react"
import GlobalContext from "../state"
import Authorized from "./Authorized"
import FinaliseModal from "./FinaliseModal"
import UserList from "./UserList"

const ChatMessages = ({ topic, messages, navigate }) => (
    <div className="chat__messages">
        <div className="chat__messages__header">
            <FontAwesomeIcon icon={faChevronLeft} size="lg" className="pointer" onClick={() => navigate("/topics")} />
            <h2>@{topic}</h2>
        </div>
        <div className="chat__messages__list" id="chat">
            {messages.filter(message => message.topic === topic).map(message =>
                <p className="chat__messages__message"><strong>{message.username}</strong> {message.text}</p>
            )}
        </div>
    </div>
)

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
            <form onSubmit={onLocalSubmit} class="form form--inline">
                <input type="text" value={message} onChange={onLocalChange}></input>
                <input type="submit" class="button" value="Send"></input>
            </form>
        </div>
    )
}

const Chat = observer(({ topic }) => {
    const [activeUsers, setActiveUsers] = useState([])
    const state = useContext(GlobalContext)
    const navigate = useNavigate()

    const [isFinaliseOpen, setIsFinaliseOpen] = useState()


    useEffect(() => {
        state.chat.connect()
            .then(() => state.chat.joinTopic(topic))
            .then(() => state.chat.getActiveUsers(topic).then(users => setActiveUsers(users)))

        state.peer.connectPeer()

        const handle = setInterval(() => {
            state.chat.getActiveUsers(topic).then(users => setActiveUsers(users))
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
            <div className="container chat">
                <FinaliseModal isOpen={isFinaliseOpen}
                    onClose={() => setIsFinaliseOpen(false)}
                    onSubmit={finaliseUser}
                    errors={state.auth.finaliseErrors} />
                <ChatMessages topic={topic} messages={state.chat.messages} navigate={navigate} />
                <ChatSender onChange={onChange} onSend={onSend} />
                <div class="chat__users">
                    <h2 class="chat__users__header">Users</h2>
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