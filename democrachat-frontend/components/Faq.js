import { Link, useNavigate } from "@reach/router"
import React from "react"

function Faq() {
    const navigate = useNavigate()

    return <div className="container container--white">
        <span className="action" onClick={() => navigate(-1)}>Back</span>
        <h2>Frequently Asked Questions</h2>
        <ul>
            <li><a href="#whatisdemocrachat">What is Democrachat?</a></li>
            <li><a href="#whatdoes00mean">What does 0G 0S mean?</a></li>
            <li><a href="#getgold">How do I get gold?</a></li>
            <li><a href="#technicalissues">I'm having a technical issue. Help!</a></li>
            <li><a href="#topicbidding">What is topic bidding?</a></li>
            <li><a href="#real">Are these real questions?</a></li>
        </ul>

        <h3 id="whatisdemocrachat">What is Democrachat</h3>
        <p>An experimental chat site about community-driven moderation.</p>

        <h3 id="whatdoes00mean">What does 0G 0S mean?</h3>
        <p>"0G" means you have 0 gold, while "0S" means you have zero silver. Gold generates over time, while silver can be used for goodies such as muting bad people and bidding on new topics.</p>

        <h3 id="getgold">How do I get gold?</h3>
        <p>Be patient and keep contributing to conversations. There are currently no standardized ways to earn gold.</p>

        <h3 id="technicalissues">I'm having a technical issue. Help!</h3>
        <p>The first thing you should try is refreshing the page. Otherwise, take a screenshot if possible and ask for help in <i>@general</i>.</p>

        <h3 id="topicbidding">What is topic bidding?</h3>
        <p>Topic bidding allows users to spending gold to suggest new topic rooms. At a certain threshold of silver, topics are automatically considered for addition to the Democrachat universe.</p>

        <h3 id="real">Are these real questions?</h3>
        <p>Good question.</p>
    </div>
}

export default Faq