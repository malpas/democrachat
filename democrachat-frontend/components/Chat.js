import { observer } from "mobx-react-lite"
import React, { useContext, useEffect, useState } from "react"
import GlobalContext from "../state"
import FinaliseModal from "./FinaliseModal"
import UserList from "./UserList"

const Chat = observer(({ topic }) => {
    const [message, setMessage] = useState("")
    const [activeUsers, setActiveUsers] = useState([])
    const state = useContext(GlobalContext)

    const [isFinaliseOpen, setIsFinaliseOpen] = useState()

    useEffect(() => {
        state.chat.connect()
            .then(() => state.chat.joinTopic(topic))
            .then(() => state.chat.getActiveUsers(topic).then(users => setActiveUsers(users)))

        const handle = setInterval(() => {
            state.chat.getActiveUsers(topic).then(users => setActiveUsers(users))
        }, 3000)

        return () => {
            clearInterval(handle)
        }
    }, [])


    const onSend = ev => {
        ev.preventDefault()
        state.chat.send(topic, message)
        setMessage("")
    }

    const finaliseUser = (username, password) => {
        state.auth.finalise(username, password).then(() => setIsFinaliseOpen(false))
    }

    return (
        <div className="container chat">
            <FinaliseModal isOpen={isFinaliseOpen}
                onClose={() => setIsFinaliseOpen(false)}
                onSubmit={finaliseUser}
                errors={state.auth.finaliseErrors} />
            {state.auth.isGuest ? (
                <strong class="action" onClick={() => setIsFinaliseOpen(true)}>Finalise your account</strong>
            ) : null}
            <div className="chat__messages">
                <h2>@{topic}</h2>
                <div className="chat__messages__list">
                    {state.chat.messages.filter(message => message.topic === topic).map(message =>
                        <p className="chat__messages__message"><strong>{message.username}</strong> {message.text}</p>
                    )}
                </div>
            </div>
            <div class="chat__send">
                <form onSubmit={onSend} class="form form--inline">
                    <input type="text" value={message} onChange={ev => { setMessage(ev.target.value) }}></input>
                    <input type="submit" class="button" value="Send"></input>
                </form>
            </div>
            <div class="chat__users">
                <h2>Users</h2>
                <UserList usernames={activeUsers} />
            </div>
        </div>
    )
})

export default Chat