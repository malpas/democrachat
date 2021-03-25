import axios from 'axios'
import React, { useEffect, useState } from 'react'

function TopicButton({ topic, onClick }) {
    const [activeUsers, setActiveUsers] = useState(null)

    const updateTopicActivity = () => {
        axios.get(`/api/topicActivity/${topic}`, null, { withCredentials: true })
            .then(resp => {
                setActiveUsers(resp.data)
            })
    }

    useEffect(updateTopicActivity)

    if (activeUsers && activeUsers.length > 0) {
        return <button className="button button--100w button--topic" onClick={onClick}>
            <span className="center">{topic}</span> <span className="right text-right mr-2">{activeUsers.length}</span>
        </button>
    }
    return <button className="button button--100w" onClick={onClick}>{topic}</button>
}

export default TopicButton