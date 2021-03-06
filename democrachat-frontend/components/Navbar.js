import { useNavigate } from "@reach/router"
import { observer } from "mobx-react-lite"
import React, { useContext, useEffect } from "react"
import GlobalContext from "../state"

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
        <p>Democrachat <strong style={{ fontWeight: "bold", cursor: "pointer" }} onClick={logout}>Logout</strong></p >
        </div>
        : null
})

export default Navbar