import { useNavigate } from "@reach/router"
import { observer } from "mobx-react-lite"
import React, { useContext, useEffect, useState } from "react"
import GlobalContext from "../state"
import logo from "url:../img/logo-dark.svg"
import { toast } from "react-toastify"

const Navbar = observer(() => {
    const state = useContext(GlobalContext)
    const navigate = useNavigate()

    useEffect(() => {
        state.auth.fetchUserInfo()
    }, [])

    const logout = () => {
        state.auth.logout()
            .then(() => navigate("/"))
            .catch(() => toast("Could not logout"))
    }

    const toggleInventory = () => state.inventory.toggleOpen()

    return (
        <div className="container navbar">
            <div className="navbar__left">
                <img src={logo} className="logo" />
            </div>
            {state.auth.username ?
                <div className="navbar__right">
                    <div>{state.auth.gold}G {state.auth.silver}S</div>
                    <button className="button button--small ml-2" onClick={toggleInventory}>Inventory</button>
                    <div style={{ fontWeight: "bold" }} class="button button--small ml-2 pointer" onClick={logout}>Logout</div>
                </div> : null}
        </div>
    )
})

export default Navbar