import { observer } from 'mobx-react-lite'
import React, { useContext, useEffect } from 'react'
import GlobalContext from '../state'

const TopicButton = observer(({ topic, onClick }) => {
    const state = useContext(GlobalContext)

    if (state.chat?.activeUsers[topic] && state.chat.activeUsers[topic].size > 0) {
        return <button className="button button--100w button--topic" onClick={onClick}>
            <span className="center">{topic}</span> <span className="right text-right mr-2">{state.chat.activeUsers[topic]?.size}</span>
        </button>
    }
    return <button className="button button--100w" onClick={onClick}>{topic}</button>
})

export default TopicButton