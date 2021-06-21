import React, { useState } from "react"

function Login({ onSubmit, onClickRegister, errorText }) {
    var [username, setUsername] = useState("")
    var [password, setPassword] = useState("")

    const submit = ev => {
        onSubmit(username, password)
        ev.preventDefault()
    }

    return <form className="form m-3" onSubmit={submit}>
        <h2 className="mt-0 mb-1">Login</h2>
        {errorText ? <p className="text-error mt-0 mb-0">{errorText}</p> : null}
        <p className="form__label">Username</p>
        <input type="text" className="form__control textbox" required onChange={ev => setUsername(ev.target.value)} value={username}></input>
        <p className="form__label">Password</p>
        <input type="password" className="form__control textbox" required onChange={ev => setPassword(ev.target.value)} value={password}></input>
        <input type="submit" className="button" value="Login"></input>
        <span className="action mt-3 text-center" onClick={onClickRegister}>1-click sign-up</span>
    </form>
}

export default Login