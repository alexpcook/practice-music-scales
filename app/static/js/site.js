let scalesDiv = document.getElementById("scales");

fetch("api/scales")
    .then(result => result.json())
    .then(jsonResult => Object.keys(jsonResult).forEach(key => {
        let header = document.createElement("h2");
        header.innerHTML = key.includes("major") ? "Major" : "Minor";
        scalesDiv.append(header);
        
        let array = jsonResult[key];
        let para = document.createElement("p");
        array.forEach(val => {
            para.innerHTML += val + ", ";
        });
        scalesDiv.append(para);
    }));
