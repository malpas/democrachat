import React, { useEffect } from 'react'
import Home from "./Home"
import GlobalContext, { RootStore } from "../state"
import { Router } from '@reach/router'
import Topics from './Topics'
import Chat from './Chat'
import Navbar from './Navbar'
import 'react-toastify/dist/ReactToastify.css';
import { ToastContainer } from 'react-toastify'
import Faq from './Faq'
import InventoryModal from './InventoryModal'

function App() {
    return (
        <GlobalContext.Provider value={new RootStore()}>
            <Router primary={false}>
                <span path="/"></span>
                <Navbar path="*" />
            </Router>
            <Router>
                <Home path="/" />
                <Faq path="/faq" />
                <Topics path="/topics" />
                <Chat path="/chat/:topic" />
            </Router>
            <ToastContainer closeOnClick={false} autoClose={40000} />
            <InventoryModal />
        </GlobalContext.Provider>
    )
}

export default App