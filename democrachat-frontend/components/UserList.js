import React from "react"

function UserList({ usernames }) {
    if (usernames.length == 0) {
        return <p>Very quiet.</p>
    }
    return (
        <div>
            <ul className="chat__userlist">
                {usernames.map(username => <li className="chat__userlist__item">{username}</li>)}
            </ul>
        </div>
    )
}

export default UserList