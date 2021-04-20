import { Link, useNavigate } from "@reach/router"
import axios from "axios"
import { observer } from "mobx-react-lite"
import React, { useContext, useEffect } from "react"
import GlobalContext from "../state"
import Authorized from "./Authorized"
import TopicBid from "./TopicBid"
import TopicButton from "./TopicButton"

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

    return <Authorized>
        <div className="container container--white mb-0">
            <div class="topics">
                <h2 className="mt-0">Topics</h2>
                <div className="topics__list">
                    {state.chat.topics.map(topic => <TopicButton topic={topic} key={topic} onClick={() => joinTopic(topic)} />)}
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
                    <p className="mt-0"><a href="/api/log">View the Transparency Log</a></p>
                    <p><Link to="/faq">Frequently Asked Questions</Link></p>
                </div>
            </div>
        </div>
    </Authorized>
})

export default Topics