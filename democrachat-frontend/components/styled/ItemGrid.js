import styled from "styled-components";

export const ItemGrid = styled.div`
    display: grid;
    grid-template-columns: 1fr 1fr;
    grid-gap: 1em;

    @media (min-width: 500px) {
        grid-template-columns: 1fr 1fr 1fr;
    }
`