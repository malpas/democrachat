import { observer } from "mobx-react-lite";
import React, { useContext } from "react"
import GlobalContext from "../state";
import UserActions from "./UserActions"

const UserLi = ({ username, isTyping, onSelect }) => {
    var className = "chat__userlist__item"
    if (isTyping) {
        className += " chat__userlist__item--typing"
    }
    return <li className={className} onClick={() => onSelect(username)}>{username}</li>
}

const UserList = observer(({ usernames, typingIndicators }) => {
    const state = useContext(GlobalContext)

    if (usernames.length == 0) {
        return <p>Very quiet.</p>
    }

    const setSelectedUsername = username => state.userActions.setSelectedUsername(username)

    return (
        <div>
            <UserActions
                username={state.userActions.selectedUsername}
                onClose={() => setSelectedUsername("")}
                callStream={state.peer.remoteStream} />
            <ul className="chat__userlist">
                {usernames.map(username => <UserLi username={username} isTyping={typingIndicators.some(ti => ti.username === username)} onSelect={setSelectedUsername} />)}
            </ul>
        </div>
    )
})

export default UserList