import React from "react"

function UserList({ usernames }) {
    return (
        <div>
            <ul className="chat__userlist">
                {usernames.map(username => <li className="chat__userlist__item">{username}</li>)}
            </ul>
        </div>
    )
}

export default UserList