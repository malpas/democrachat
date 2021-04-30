import { Link, useNavigate } from "@reach/router"
import React from "react"
import { Helmet } from "react-helmet"

function Faq() {
    const navigate = useNavigate()

    return <div className="container container--white mb-0">
        <Helmet>
            <title>Democrachat - FAQ</title>
        </Helmet>
        <span className="action" onClick={() => navigate(-1)}>Back</span>
        <h2>Frequently Asked Questions</h2>
        <ul>
            <li><a href="#whatisdemocrachat">What is Democrachat?</a></li>
            <li><a href="#whatdoes00mean">What does 0G 0S mean?</a></li>
            <li><a href="#useofsilver">What can I do with silver?</a></li>
            <li><a href="#getgold">How do I get gold?</a></li>
            <li><a href="#kudo">What is a kudo?</a></li>
            <li><a href="#technicalissues">I'm having a technical issue. Help!</a></li>
            <li><a href="#topicbidding">What is topic bidding?</a></li>
        </ul>

        <h3 id="whatisdemocrachat">What is Democrachat?</h3>
        <p>An experimental chat site about community-driven moderation.</p>

        <h3 id="whatdoes00mean">What does 0G 0S mean?</h3>
        <p>"0G" means you have 0 gold, while "0S" means you have zero silver. Gold generates over time, while silver can be used for goodies such as muting bad people and bidding on new topics.</p>

        <h3 id="useofsilver">What can I do with silver?</h3>
        <p>Silver is mostly used for muting people and topic bidding.</p>

        <h3 id="getgold">How do I get gold?</h3>
        <p>Be patient and keep contributing to conversations. There are currently no standardized ways to earn gold.</p>

        <h3 id="kudo">What is a kudo?</h3>
        <p>A "kudo" is a random gift. Reward people and form trust by sending them. Your account must not be recent to send kudos.</p>

        <h3 id="technicalissues">I'm having a technical issue. Help!</h3>
        <p>The first thing you should try is refreshing the page. Otherwise, take a screenshot if possible and ask for help in <i>@general</i>.</p>

        <h3 id="topicbidding">What is topic bidding?</h3>
        <p>Topic bidding allows users to spending gold to suggest new topic rooms. At a certain threshold of silver, topics are automatically considered for addition to the Democrachat universe.</p>
    </div>
}

export default Faq