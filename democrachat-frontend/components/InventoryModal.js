import { observer } from "mobx-react-lite";
import React, { useContext, useEffect } from "react"
import GlobalContext from "../state";
import InventoryItems from "./InventoryItems";

const InventoryModal = observer(() => {
    const state = useContext(GlobalContext)

    const toggleOpen = () => state.inventory.toggleOpen()

    const onCloseOutside = ev => {
        var style = window.getComputedStyle(ev.target)
        if (style.zIndex != 999) return
        toggleOpen()
    }

    useEffect(() => {
        if (state.auth.username)
            state.inventory.getInventory()
    }, [])

    const useItem = uuid => {
        state.inventory.useItem(uuid)
    }

    if (!state.inventory.isOpen) {
        return null;
    }
    return (
        <div className="modal__wrapper" onClick={onCloseOutside}>
            <div className="modal__inner">
                <h2 className="modal__title">Inventory</h2>
                <button className="button modal__close" onClick={toggleOpen}>X</button>
                <div className="modal__content">
                    <p className="text-success mt-0 mb-1">{state.inventory.message}</p>
                    <InventoryItems items={state.inventory.items} useItem={useItem} />
                </div>
            </div>
        </div>
    )
})

export default InventoryModal