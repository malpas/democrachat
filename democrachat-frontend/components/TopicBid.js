import React, { useState } from "react"

function TopicBid({ onBid, errors, result }) {
    const [name, setName] = useState("")
    const [silver, setSilver] = useState(0)

    const onSubmit = ev => {
        ev.preventDefault()
        onBid(name, silver)
    }

    return (
        <form className="form" onSubmit={onSubmit}>
            {result ? <p className="text-success mt-0">{result}</p> : null}
            {errors.length > 0 ?
                <ul className="mt-0">
                    {errors.map(error => <li className="text-error" key={error}>{error}</li>)}
                </ul> : null}
            <p className="form__label mt-0">Topic Name</p>
            <input type="text" className="form__control textbox" placeholder="e.g. architecture" value={name} onChange={ev => setName(ev.target.value)} />
            <p className="form__label">Silver</p>
            <input type="number" className="form__control textbox" value={silver} onChange={ev => setSilver(ev.target.value)} />
            <input type="submit" className="button button--secondary" value="Bid"></input>
        </form>
    )
}

export default TopicBid