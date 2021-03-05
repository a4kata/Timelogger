"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var React = require("react");
require("../custom.css");
var Logs = function (props) {
    return (React.createElement("div", { className: "popup-box" },
        React.createElement("div", { className: "box" },
            React.createElement("span", { className: "close-icon", onClick: props.handleClose }, "x"),
            props.content)));
};
exports.default = Logs;
//# sourceMappingURL=Logs.js.map