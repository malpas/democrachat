import { observer } from "mobx-react-lite"
import React, { useContext, useEffect, useState } from "react"
import GlobalContext from "../state"

const UserActions = observer(({ username, onClose }) => {
    const state = useContext(GlobalContext)


    const videoRef = React.createRef()

    const [silver, setSilver] = useState(0)

    const muteUser = ev => {
        state.userActions.muteUser(username, silver)
        ev.preventDefault()
    }

    const callUser = () => {
        state.peer.call(username)
    }

    const updateVideo = () => {
        if (videoRef.current && videoRef.current.srcObject != state.peer.remoteStream) {
            videoRef.current.srcObject = state.peer.remoteStream
            videoRef.current.play()
        }
    }
    useEffect(() => updateVideo())

    const videoChatButton = state.peer.currentCall
        ? <button className="button button--100w" onClick={() => state.peer.endCall()}>End Chat</button>
        : <button className="button button--100w" disabled={!state.peer.isConnected} onClick={() => callUser()}>Video Chat</button>


    if (!username) return null

    return <div class="user-actions">
        <div class="user-actions__header">
            <h3>{username}</h3>
            <p onClick={onClose} className="pointer">X</p>
        </div>
        {state.peer.currentCall ? <video ref={videoRef} style={{ maxWidth: "100%" }} /> : null}
        <div class="user-actions__errors">
            {state.userActions.userActionResult ? <p className="text-success">{state.userActions.userActionResult}</p> : null}
            {state.userActions.userActionErrors ?
                <ul className="text-error">
                    {state.userActions.userActionErrors.map(error => <li>{error}</li>)}
                </ul>
                : null}
        </div>
        <div class="user-actions__mute">
            <form className="form form--inline" onSubmit={muteUser}>
                <input type="number" className="textbox" placeholder="silver" onChange={ev => setSilver(ev.target.value)} value={silver} />
                <input type="submit" className="button" value="Mute" />
            </form>
        </div>
        <div className="user-actions__chat">
            {username != state.auth.username ? videoChatButton : null}
        </div>
    </div>
})

export default UserActions