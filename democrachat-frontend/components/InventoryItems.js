import React from "react"
import { ItemCard, ItemCardButtons, ItemCardContent, ItemCardImage, ItemCardName } from "./styled/ItemCard"
import { ItemGrid } from "./styled/ItemGrid"

function InventoryItems({ items, useItem }) {
    if (!items || items.length == 0) return <p>No items.</p>
    return <ItemGrid>
        {items.map(item =>
            <ItemCard>
                <ItemCardContent>
                    <ItemCardImage src={item.imageSrc} />
                </ItemCardContent>
                <ItemCardName>{item.name}</ItemCardName>
                <ItemCardButtons>
                    <button
                        className="button button--100w mt-0"
                        onClick={() => useItem(item.uuid)}>Use</button>
                </ItemCardButtons>
            </ItemCard>)}
    </ItemGrid>

}

export default InventoryItems