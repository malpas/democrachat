import { useNavigate } from "@reach/router"
import { observer } from "mobx-react-lite"
import React, { useContext, useEffect } from "react"
import GlobalContext from "../state"
import logo from "url:../img/logo-dark.svg"

const Navbar = observer(() => {
    const state = useContext(GlobalContext)
    const navigate = useNavigate()

    useEffect(() => {
        state.auth.fetchUserInfo()
    }, [])

    const logout = () => {
        state.auth.logout()
        navigate("/")
    }

    return state.auth.username ?
        <div className="container">
            <img src={logo} className="logo" />
            <strong style={{ fontWeight: "bold", cursor: "pointer", marginLeft: "1em" }} onClick={logout}>Logout</strong>
        </div>
        : null
})

export default Navbar