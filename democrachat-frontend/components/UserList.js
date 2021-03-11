import { observer } from "mobx-react-lite";
import React, { useContext, useState } from "react"
import GlobalContext from "../state";
import TopicBid from "./TopicBid";
import UserActions from "./UserActions"

const UserList = observer(({ usernames }) => {
    const [selectedUsername, setSelectedUsername] = useState(null);
    const state = useContext(GlobalContext)

    if (usernames.length == 0) {
        return <p>Very quiet.</p>
    }

    const muteUser = (username, silver) => {
        state.userActions.muteUser(username, silver)
    }

    return (
        <div>
            <UserActions
                username={selectedUsername}
                onClose={() => setSelectedUsername("")} onMute={muteUser}
                errors={state.userActions.userActionErrors}
                resultMessage={state.userActions.userActionResult} />
            <ul className="chat__userlist">
                {usernames.map(username => <li className="chat__userlist__item" onClick={() => setSelectedUsername(username)} key={username}>{username}</li>)}
            </ul>
        </div>
    )
})

export default UserList