import React, { useState } from "react"

function Login({ onSubmit, onClickRegister, errorText }) {
    var [username, setUsername] = useState("")
    var [password, setPassword] = useState("")

    const submit = ev => {
        onSubmit(username, password)
        ev.preventDefault()
    }

    return <form class="form m-3" onSubmit={submit}>
        <h2>Login</h2>
        {errorText ? <p class="text-error mt-0 mb-0">{errorText}</p> : null}
        <p class="form__label">Username</p>
        <input type="text" class="form__control" required onChange={ev => setUsername(ev.target.value)} value={username}></input>
        <p class="form__label">Password</p>
        <input type="password" class="form__control" required onChange={ev => setPassword(ev.target.value)} value={password}></input>
        <input type="submit" class="form__submit" value="Login"></input>
        <span class="action mt-3 text-center" onClick={onClickRegister}>1-click sign-up</span>
    </form>
}

export default Login