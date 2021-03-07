import React from "react"

function UserList({ usernames }) {
    return (
        <div>
            <ul>
                {usernames.map(username => <li>{username}</li>)}
            </ul>
        </div>
    )
}

export default UserList