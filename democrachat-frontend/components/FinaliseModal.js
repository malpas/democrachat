import React, { useState } from "react";

const styles = {

}

const FinaliseModal = ({ isOpen, onClose, onSubmit, errors }) => {
    if (!isOpen) return null

    const [username, setUsername] = useState("")
    const [password, setPassword] = useState("")

    const submit = ev => {
        ev.preventDefault()
        onSubmit(username, password)
    }

    const onCloseOutside = ev => {
        var style = window.getComputedStyle(ev.target)
        if (style.zIndex != 999) return
        onClose()
    }

    return <div className="modal__wrapper" onClick={onCloseOutside}>
        <div className="modal__inner">
            <h2 className="modal__title">Finalise Account</h2>
            <button className="button modal__close" onClick={onClose}>X</button>
            <div className="modal__content">
                <p>Get your real username, and a password too.</p>
                {errors ? <ul className="text-error">
                    {errors.map(error => <li>{error}</li>)}
                </ul> : null}
                <form className="form" onSubmit={submit}>
                    <p className="form__label"><label>Username</label></p>
                    <input className="form__control" required type="text" value={username} onChange={ev => setUsername(ev.target.value)}></input>
                    <p className="form__label"><label>Password</label></p>
                    <input className="form__control" required type="password" value={password} onChange={ev => setPassword(ev.target.value)}></input>
                    <input className="button" type="submit" value="Finalise"></input>
                </form>
            </div>
        </div>
    </div>
}

export default FinaliseModal