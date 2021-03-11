import { useNavigate } from "@reach/router"
import { observer } from "mobx-react-lite"
import React, { useContext, useEffect } from "react"
import GlobalContext from "../state"

const Topics = observer(() => {
    const state = useContext(GlobalContext)
    const navigate = useNavigate()

    useEffect(() => {
        state.chat.getTopics()
        state.chat.connect()
    }, [])

    const joinTopic = topic => {
        navigate(`/chat/${topic}`)
    }

    return <div class="container">
        <h2>Topics</h2>
        {state.chat.topics.map(topic => <button class="button" key={topic} onClick={() => joinTopic(topic)}>{topic}</button>)}
        <p><a href="/api/log">View the Castle Transparency Log ™</a></p>
    </div>
})

export default Topics