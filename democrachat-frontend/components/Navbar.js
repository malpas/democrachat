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
                    <span>{state.auth.gold}G {state.auth.silver}S</span>
                    <button className="button button--small ml-2 mt-0" onClick={toggleInventory}>Inventory</button>
                    <div class="button button--small ml-2 mt-0 pointer" onClick={logout}>Logout</div>
                </div> : null}
        </div>
    )
})

export default Navbar