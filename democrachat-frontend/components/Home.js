import React, { useContext, useEffect } from "react";
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

    useEffect(() => {
        state.auth.fetchUserInfo().then(() => {
            if (state.auth.username) {
                navigate("/topics")
            }
        })
    }, [])

    return (
        <div className="home-bg">
            <div className="vertical-center-wrap">
                <div className="home container">
                    <div className="home__header">
                        <img src={logo} className="logo mb-3" alt="Democrachat" />
                    </div>
                    <div className="home__blurb text-white">
                        <span className="dramatic-text mt-0">The chat platform for hip people</span>
                        <p>Democrachat is an experimental, semi-democratized chat site. See the <Link to="/faq" className="mt-1 home__faq">FAQ</Link> for more info.</p>
                    </div>
                    <div className="home__login">
                        <Login onSubmit={(username, password) => login(username, password)}
                            onClickRegister={register}
                            errorText={state.auth.errorText} />
                    </div>
                </div >
            </div>
        </div>
    )
})

export default Home