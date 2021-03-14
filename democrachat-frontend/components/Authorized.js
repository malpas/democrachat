import { useNavigate } from "@reach/router"
import { observer } from "mobx-react-lite"
import React, { useContext } from "react"
import GlobalContext from "../state"

const Authorized = observer(({ children }) => {
    const navigate = useNavigate()
    const state = useContext(GlobalContext)

    if (!state.auth.username)
        state.auth.fetchUserInfo().catch(() => navigate("/"))
    return children
})

export default Authorized