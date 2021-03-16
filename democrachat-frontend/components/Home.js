import React, { useContext } from "react";
import Login from "./Login"
import "../node_modules/normalize.css/normalize.css"
import "../css/default.css"
import { observer } from "mobx-react-lite";
import GlobalContext from "../state";
import { Link, useNavigate } from "@reach/router";
import logo from "url:../img/logo-light.svg"

const Home = observer(() => {
    const state = useContext(GlobalContext)
    const navigate = useNavigate()

    const setup = () => {
        navigate("/topics")
        state.chat.connect()
    }

    const login = (username, password) => {
        state.auth.login(username, password)
            .then(() => setup())
    }

    const register = () => {
        state.auth.register()
            .then(() => setup())
    }

    return (
        <div class="home-bg">
            <div class="vertical-center-wrap">
                <div class="home">
                    <div class="home__header">
                        <img src={logo} class="logo mb-2" alt="Democrachat" />
                    </div>
                    <div class="home__blurb text-white">
                        <span class="dramatic-text">The chat platform for hip people.</span>
                    </div>
                    <div class="home__login">
                        <Login onSubmit={(username, password) => login(username, password)}
                            onClickRegister={register}
                            errorText={state.auth.errorText} />
                    </div>
                    <Link to="/faq" className="mt-1 home__faq">FAQ</Link>
                </div >
            </div>
        </div>
    )
})

export default Home