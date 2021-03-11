import React, { useState } from "react"

function UserActions({ username, errors, resultMessage, onClose, onMute }) {
    if (!username) return null

    const [silver, setSilver] = useState(0)

    return <div class="user-actions">
        <div class="user-actions__header">
            <h3>{username}</h3>
            <p onClick={onClose}>X</p>
        </div>
        <div class="user-actions__errors">
            {resultMessage ? <p className="text-success">{resultMessage}</p> : null}
            {errors ?
                <ul className="text-error">
                    {errors.map(error => <li>{error}</li>)}
                </ul>
                : null}
        </div>
        <div class="user-actions__mute">
            <input type="number" placeholder="silver" onChange={ev => setSilver(ev.target.value)} value={silver} />
            <strong class="button" onClick={() => onMute(username, silver)}>Mute</strong>
        </div>
    </div>
}

export default UserActions