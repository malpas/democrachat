import { useNavigate } from "@reach/router"
import { observer } from "mobx-react-lite"
import React, { useContext, useEffect } from "react"
import GlobalContext from "../state"
import TopicBid from "./TopicBid"

const Topics = observer(() => {
    const state = useContext(GlobalContext)
    const navigate = useNavigate()

    useEffect(() => {
        state.chat.getTopics()
        state.chat.connect()
    }, [])

    const onBid = (name, silver) => {
        state.userActions.topicBid(name, silver)
    }

    const joinTopic = topic => {
        navigate(`/chat/${topic}`)
    }

    return <div className="container">
        <h2>Topics</h2>
        <div class="topics">
            <div className="topics__list">
                {state.chat.topics.map(topic => <button class="button button--100w" key={topic} onClick={() => joinTopic(topic)}>{topic}</button>)}
            </div>
            {state.auth.silver > 0 ?
                <div className="topics__bid">
                    <div className="bid__info">
                        <h3 className="mb-0 mt-0">Bid</h3>
                        <p className="mb-1 mt-2">You could bid for a new topic using your silver.</p>
                    </div>
                    <div className="bid__form">
                        <TopicBid onBid={onBid} errors={state.userActions.bidErrors} result={state.userActions.bidResult} />
                    </div>
                </div> : null}
            <div className="topics__log">
                <a href="/api/log">View the Transparency Log</a>
            </div>
        </div>
    </div>
})

export default Topics