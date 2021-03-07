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
        <div className="container">
            <FinaliseModal isOpen={isFinaliseOpen}
                onClose={() => setIsFinaliseOpen(false)}
                onSubmit={finaliseUser}
                errors={state.auth.finaliseErrors} />
            {state.auth.isGuest ? (
                <strong class="action" onClick={() => setIsFinaliseOpen(true)}>Finalise your account</strong>
            ) : null}
            <div>
                {state.chat.messages.filter(message => message.topic === topic).map(message =>
                    <p><strong>{message.username}</strong> {message.text}</p>
                )}
            </div>
            <form onSubmit={onSend} class="form">
                <input type="text" value={message} onChange={ev => { setMessage(ev.target.value) }}></input>
                <input type="submit" class="button" value="Send"></input>
            </form>
            <UserList usernames={activeUsers} />
        </div>
    )
})

export default Chat