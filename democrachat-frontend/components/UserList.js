import { observer } from "mobx-react-lite";
import React, { useContext, useEffect, useState } from "react"
import GlobalContext from "../state";
import UserActions from "./UserActions"

const UserList = observer(({ usernames }) => {
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
                {usernames.map(username => <li className="chat__userlist__item" onClick={() => setSelectedUsername(username)} key={username}>{username}</li>)}
            </ul>
        </div>
    )
})

export default UserList