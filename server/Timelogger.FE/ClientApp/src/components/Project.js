"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var React = require("react");
var react_redux_1 = require("react-redux");
var RegTime_1 = require("../components/RegTime");
var Logs_1 = require("../components/Logs");
require("../custom.css");
var ProjectComponent = function () { return (React.createElement("div", null,
    React.createElement("h1", null, "Projects"),
    LoadProjects())); };
function LoadProjects() {
    var _a = React.useState(0), minutes = _a[0], SetMinutes = _a[1];
    var _b = React.useState(""), notes = _b[0], SetNotes = _b[1];
    var _c = React.useState([]), logs = _c[0], SetLogs = _c[1];
    var _d = React.useState(0), projectID = _d[0], SetprojectID = _d[1];
    var _e = React.useState(false), isRegOpen = _e[0], setIsRegOpen = _e[1];
    var toggleRegtime = function (id) { setIsRegOpen(!isRegOpen); SetprojectID(id); };
    var _f = React.useState(false), isLogOpen = _f[0], setIsLogOpen = _f[1];
    var toggleLogs = function (logs) { setIsLogOpen(!isLogOpen); SetLogs(logs); };
    var _g = React.useState([]), projects = _g[0], SetProjects = _g[1];
    var url = "https://localhost:44358/api/Projects/GetProjects_OrderByDeadline";
    fetch(url)
        .then(function (response) { return response.json(); })
        .then(function (data) {
        SetProjects(data);
    });
    var url2 = "https://localhost:44358/api/Projects/RegisterTime";
    function Registertime() {
        if (minutes < 30) {
            alert("Time has to be more than 30 minutes");
        }
        else {
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
    return (React.createElement("table", { className: 'table table-striped', "aria-labelledby": "tabelLabel" },
        React.createElement("thead", null,
            React.createElement("tr", null,
                React.createElement("th", null, "ID"),
                React.createElement("th", null, "Name"),
                React.createElement("th", null, "Client"),
                React.createElement("th", null, "Deadline"),
                React.createElement("th", null, "Hour rate"),
                React.createElement("th", null, "Total time"),
                React.createElement("th", null, "Cost"),
                React.createElement("th", null, "State"),
                React.createElement("th", null),
                React.createElement("th", null))),
        React.createElement("tbody", null, projects.map(function (pro) {
            return React.createElement("tr", { key: pro.id },
                React.createElement("td", null, pro.id),
                React.createElement("td", null, pro.name),
                React.createElement("td", null, pro.clientName),
                React.createElement("td", null, new Date(pro.deadline).toLocaleDateString()),
                React.createElement("td", null, pro.hourRate),
                React.createElement("td", null, pro.time),
                React.createElement("td", null, pro.cost),
                React.createElement("td", null, pro.completed ? React.createElement("label", null, "Closed") : React.createElement("label", null, "Open")),
                React.createElement("td", null,
                    React.createElement("button", { onClick: function () { return toggleRegtime(pro.id); } }, "Register time")),
                isRegOpen && React.createElement(RegTime_1.default, { content: React.createElement(React.Fragment, null,
                        React.createElement("div", null,
                            React.createElement("div", { className: "leftF" },
                                React.createElement("b", null, "Register time on project")),
                            React.createElement("div", { className: "leftF" },
                                "Time spent: ",
                                React.createElement("input", { onChange: function (event) { return SetMinutes(Number(event.target.value)); }, type: "number", min: "1", step: "1", id: "minutes", required: true })),
                            React.createElement("div", { className: "leftF textArea" },
                                React.createElement("textarea", { onChange: function (event) { return SetNotes(event.target.value); }, placeholder: "Note to the client...", className: "textArea", required: true })),
                            React.createElement("div", { className: "rightF" },
                                React.createElement("button", { onClick: Registertime }, "Register")))), handleClose: toggleRegtime }),
                React.createElement("td", null,
                    React.createElement("button", { hidden: pro.logs == null, onClick: function () { return toggleLogs(pro.logs); } }, "Show Logs")),
                isLogOpen && React.createElement(Logs_1.default, { content: React.createElement(React.Fragment, null, logs.map(function (log) {
                        return React.createElement("div", null,
                            React.createElement("div", { className: "leftF" },
                                "Time: ",
                                React.createElement("input", { type: "text", value: log.time, readOnly: true })),
                            React.createElement("div", { className: "leftF" },
                                "Notes: ",
                                React.createElement("textarea", { value: log.note, readOnly: true })));
                    })), handleClose: toggleLogs }));
        }))));
}
exports.default = react_redux_1.connect()(ProjectComponent);
//# sourceMappingURL=Project.js.map