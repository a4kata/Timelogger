import * as React from 'react';
import { connect } from 'react-redux';
import RegTime from '../components/RegTime';
import Logs from '../components/Logs';
import '../custom.css';

export interface Log {
    id: number;
    note: string;
    time: number;
}

export interface Project {
    id: number;
    name: string;
    clientName: string;
    deadline: Date;
    hourRate: number;
    time: string;
    cost: number;
    completed: boolean;
    logs: Log[];
}
const ProjectComponent = () => (

    <div>
        <h1>Projects</h1>
        {LoadProjects()}
    </div>
);

function LoadProjects() {
    const [minutes, SetMinutes] = React.useState(0);
    const [notes, SetNotes] = React.useState("");
    const [logs, SetLogs] = React.useState([]);
    const [projectID, SetprojectID] = React.useState(0);
    const [isRegOpen, setIsRegOpen] = React.useState(false);
    const toggleRegtime = (id) => { setIsRegOpen(!isRegOpen); SetprojectID(id); }
    const [isLogOpen, setIsLogOpen] = React.useState(false);
    const toggleLogs = (logs) => { setIsLogOpen(!isLogOpen); SetLogs(logs) }
    const [projects, SetProjects] = React.useState([]);
    const url = "https://localhost:44358/api/Projects/GetProjects_OrderByDeadline";
    fetch(url)
        .then(response => response.json() as Promise<Project[]>)
        .then(data => {
            SetProjects(data)
        });
    const url2 = "https://localhost:44358/api/Projects/RegisterTime";
    function Registertime() {
        if (minutes < 30) {
            alert("Time has to be more than 30 minutes");
        } else {
            fetch(url2, {
                method: "POST",
                headers: {
                    "Accept": "application/json",
                    "Content-Type": "application/json",
                },
                body: JSON.stringify({
                    projectID: projectID,
                    minutes: minutes,
                    notes: notes
                })
            });
            setIsRegOpen(false);
        }
    }
    return (
        <table className='table table-striped' aria-labelledby="tabelLabel">
            <thead>
                <tr>
                    <th>ID</th>
                    <th>Name</th>
                    <th>Client</th>
                    <th>Deadline</th>
                    <th>Hour rate</th>
                    <th>Total time</th>
                    <th>Cost</th>
                    <th>State</th>
                    <th></th>
                    <th></th>
                </tr>
            </thead>
            <tbody>
                {projects.map((pro: Project) =>
                    <tr key={pro.id}>
                        <td>{pro.id}</td>
                        <td>{pro.name}</td>
                        <td>{pro.clientName}</td>
                        <td>{new Date(pro.deadline).toLocaleDateString()}</td>
                        <td>{pro.hourRate}</td>
                        <td>{pro.time}</td>
                        <td>{pro.cost}</td>
                        <td>{pro.completed ? <label>Closed</label> : <label>Open</label>}</td>
                        <td><button onClick={() => toggleRegtime(pro.id)}>Register time</button></td>
                        {isRegOpen && <RegTime
                            content={<>
                                <div>
                                    <div className="leftF"><b>Register time on project</b></div>
                                    <div className="leftF">Time spent: <input onChange={event => SetMinutes(Number(event.target.value))} type="number" min="1" step="1" id="minutes" required /></div>
                                    <div className="leftF textArea"><textarea onChange={event => SetNotes(event.target.value)} placeholder="Note to the client..." className="textArea" required /></div>
                                    <div className="rightF"><button onClick={Registertime}>Register</button></div>
                                </div>
                            </>}
                            handleClose={toggleRegtime}
                        />}
                        <td><button hidden={pro.logs == null } onClick={() => toggleLogs(pro.logs)}>Show Logs</button></td>
                        {isLogOpen && <Logs
                            content={<>
                                {logs.map((log: Log) =>
                                    <div>
                                        <div className="leftF">Time: <input type="text"  value={log.time} readOnly /></div>
                                        <div className="leftF">Notes: <textarea value={log.note} readOnly /></div>
                                    </div>)}
                            </>}
                            handleClose={toggleLogs}
                        />}
                    </tr>
                )}
            </tbody>
        </table>
    );
}


export default connect()(ProjectComponent);