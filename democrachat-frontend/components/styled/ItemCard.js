import styled from "styled-components"

export const ItemCard = styled.div`
    display: grid;
    grid-template-areas:
        "content content"
        "name name"
        "buttons buttons";
    grid-template-columns: 1fr 1fr;
    grid-template-rows: auto auto auto;
    align-items: center;
`

export const ItemCardContent = styled.div`
    grid-area: content;
    display: flex;
    justify-content: center;
`

export const ItemCardName = styled.span`
    grid-area: name;
    padding-top: 0.25em;
    font-weight: bold;
    height: 2em;
    display: flex;
    justify-content: center;
    align-items: center;
`

export const ItemCardButtons = styled.div`
    grid-area: buttons;
    display: flex;
    justify-content: flex-end;
    height: 2em;
    margin-bottom: 1em;
`

export const ItemCardImage = styled.img`
    width: 7em;
`