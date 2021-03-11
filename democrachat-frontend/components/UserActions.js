import React, { useState } from "react"

function UserActions({ username, errors, resultMessage, onClose, onMute }) {
    if (!username) return null

    const [silver, setSilver] = useState(0)

    const submit = ev => {
        ev.preventDefault()
        onMute(username, silver)
    }

    return <div class="user-actions">
        <div class="user-actions__header">
            <h3>{username}</h3>
            <p onClick={onClose} className="pointer">X</p>
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
            <form className="form form--inline" onSubmit={submit}>
                <input type="number" placeholder="silver" onChange={ev => setSilver(ev.target.value)} value={silver} />
                <input type="submit" className="form__submit" value="Mute" />
            </form>
        </div>
    </div>
}

export default UserActions