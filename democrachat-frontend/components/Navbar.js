import { useNavigate } from "@reach/router"
import { observer } from "mobx-react-lite"
import React, { useContext, useEffect, useState } from "react"
import GlobalContext from "../state"
import logo from "url:../img/logo-dark.svg"
import FinaliseModal from "./FinaliseModal"

const Navbar = observer(() => {
    const state = useContext(GlobalContext)
    const navigate = useNavigate()

    useEffect(() => {
        state.auth.fetchUserInfo()
    }, [])

    const logout = () => {
        state.auth.logout()
            .then(() => navigate("/"))
    }

    return (
        <div className="container navbar">
            <div className="navbar__left">
                <img src={logo} className="logo" />
            </div>
            {state.auth.username ?
                <div className="navbar__right">
                    <div>{state.auth.gold}G {state.auth.silver}S</div>
                    <div style={{ fontWeight: "bold", cursor: "pointer", marginLeft: "1em" }} class="button button--small" onClick={logout}>Logout</div>
                </div> : null}
        </div>
    )
})

export default Navbar