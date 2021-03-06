import { JsonHubProtocol } from "@microsoft/signalr"
import { observer } from "mobx-react-lite"
import React, { useContext, useEffect, useState } from "react"
import GlobalContext from "../state"

const Chat = observer(({ topic }) => {
    const [message, setMessage] = useState("")
    const state = useContext(GlobalContext)

    useEffect(() => {
        state.chat.connect()
            .then(() => state.chat.joinTopic(topic))
    }, [])

    const onSend = ev => {
        ev.preventDefault()
        state.chat.send(topic, message)
        setMessage("")
    }

    return (
        <div>
            <form onSubmit={onSend} class="form">
                <input type="text" value={message} onChange={ev => { setMessage(ev.target.value) }}></input>
                <input type="submit" class="button" value="Send"></input>
            </form>
            <div>
                {state.chat.messages.filter(message => message.topic === topic).map(message =>
                    <p><strong>{message.username}</strong> {message.text}</p>
                )}
            </div>
        </div>
    )
})

export default Chat