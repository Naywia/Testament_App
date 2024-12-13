// wwwroot/js/scripts.js

window.addClassToBody = function (className) {
    document.body.classList.add(className);
};

window.removeClassFromBody = function (className) {
    document.body.classList.remove(className);
};

function downloadFile(fileBytes, fileName) {
    // Create a Blob from the file data (PDF content)
    var blob = new Blob([fileBytes], { type: "application/pdf" });

    // Create a URL for the Blob object
    var url = window.URL.createObjectURL(blob);

    // Create an invisible download link
    var link = document.createElement('a');
    link.href = url;
    link.download = fileName;

    // Programmatically click the link to trigger the download
    link.click();

    // Clean up by revoking the object URL after the download is triggered
    window.URL.revokeObjectURL(url);
}

// The code has to reference HTML elements and add people to their respective elements after generating them.
function addFamilyTree(id, html) {
    document.getElementById(id).innerHTML = html;
}

function updateFamilyTree() {
    const data = [
        { "id": 1, "name": "Me", "children": [4], "partners": [2, 3], root: true, level: 0, "parents": [8, 9] },
        { "id": 2, "name": "Mistress", "children": [4], "partners": [1], level: 0, "parents": [] },
        { "id": 3, "name": "Wife", "children": [5], "partners": [1], level: 0, "parents": [] },
        { "id": 4, "name": "Son", "children": [], "partners": [], level: -1, "parents": [1, 2] },
        { "id": 5, "name": "Daughter", "children": [7], "partners": [6], level: -1, "parents": [1, 3] },
        { "id": 6, "name": "Boyfriend", "children": [7], "partners": [5], level: -1, "parents": [] },
        { "id": 7, "name": "Son Last", "children": [], "partners": [], level: -2, "parents": [5, 6] },
        { "id": 8, "name": "Jeff", "children": [1], "partners": [9], level: 1, "parents": [10, 11] },
        { "id": 9, "name": "Maggie", "children": [1], "partners": [8], level: 1, "parents": [13, 14] },
        { "id": 10, "name": "Bob", "children": [8], "partners": [11], level: 2, "parents": [12] },
        { "id": 11, "name": "Mary", "children": [], "partners": [10], level: 2, "parents": [] },
        { "id": 12, "name": "John", "children": [10], "partners": [], level: 3, "parents": [] },
        { "id": 13, "name": "Robert", "children": [9], "partners": [14], level: 2, "parents": [] },
        { "id": 14, "name": "Jessie", "children": [9], "partners": [13], level: 2, "parents": [15, 16] },
        { "id": 15, "name": "Raymond", "children": [14], "partners": [16], level: 3, "parents": [] },
        { "id": 16, "name": "Betty", "children": [14], "partners": [15], level: 3, "parents": [] },
    ];

    const elements = [],
          levels = [],
          levelMap = [],
          tree = document.getElementById('familyTree'),
          gap = 32,
          size = 64;

    let startTop = 0;
    let startLeft = 0;

    // Extract unique levels and sort them
    data.forEach(elem => {
        if (!levels.includes(elem.level)) {
            levels.push(elem.level);
        }
    });
    levels.sort((a, b) => a - b);

    // Process each level and plot nodes
    levels.forEach(level => {
        const levelNodes = data.filter(person => person.level === level);

        levelNodes.forEach(person => {
            plotNode(person, 'self');
            plotParents(person);
        });
    });

    adjustNegatives();

    /* Functions */
    function plotParents(person) {
        if (!person || !person.parents) return;

        person.parents.reduce((previousId, currentId) => {
            const previousParent = findPerson(previousId);
            const currentParent = findPerson(currentId);

            plotNode(currentParent, 'parents', person, person.parents.length);

            if (previousParent) {
                plotConnector(previousParent, currentParent, 'partners');
            }

            plotConnector(person, currentParent, 'parents');
            plotParents(currentParent);

            return currentId;
        }, null);
    }

    function plotNode(person, type, relative = null, parentCount = 0) {
        if (document.getElementById(person.id)) return;

        const node = document.createElement('div');
        node.id = person.id;
        node.classList.add('node', 'asset');
        node.textContent = person.name;
        node.setAttribute('data-level', person.level);

        let relativeNode;
        const levelData = findLevel(person.level) || { level: person.level, top: startTop };
        if (!levelMap.includes(levelData)) levelMap.push(levelData);

        switch (type) {
            case 'self':
                node.style.left = `${startLeft}px`;
                node.style.top = `${levelData.top}px`;
                break;
            case 'partners':
                relativeNode = document.getElementById(relative.id);
                node.style.left = `${parseInt(relativeNode.style.left) + size + gap * 2}px`;
                node.style.top = `${relativeNode.style.top}`;
                break;
            case 'parents':
                relativeNode = document.getElementById(relative.id);
                const offset = parentCount === 1 ? 0 : -size;
                node.style.left = `${parseInt(relativeNode.style.left) + offset}px`;
                node.style.top = `${parseInt(relativeNode.style.top) - gap - size}px`;
                break;
        }

        while (detectCollision(node)) {
            node.style.left = `${parseInt(node.style.left) + size + gap * 2}px`;
        }

        levelData.top = Math.min(levelData.top, parseInt(node.style.top));
        elements.push({ id: node.id, left: parseInt(node.style.left), top: parseInt(node.style.top) });
        tree.appendChild(node);
    }

    function plotConnector(source, destination, relation) {
        const connectorId = `${source.id}-${destination.id}`;
        if (document.getElementById(connectorId)) return;

        const connector = document.createElement('div');
        connector.id = connectorId;
        connector.classList.add('asset', 'connector', relation === 'partners' ? 'h' : 'v');

        const [x1, y1, x2, y2] = [
            parseInt(source.style.left),
            parseInt(source.style.top),
            parseInt(destination.style.left),
            parseInt(destination.style.top)
        ];

        if (relation === 'partners') {
            connector.style.width = `${x2 - x1}px`;
            connector.style.left = `${x1 + size}px`;
            connector.style.top = `${y1 + size / 2}px`;
        } else {
            const length = Math.hypot(x2 - x1, y2 - y1);
            const angle = Math.atan2(y2 - y1, x2 - x1) * (180 / Math.PI);

            connector.style.width = `${length}px`;
            connector.style.left = `${x1 + size / 2}px`;
            connector.style.top = `${y1}px`;
            connector.style.transform = `rotate(${angle}deg)`;
        }

        tree.appendChild(connector);
    }

    function adjustNegatives() {
        const allNodes = [...document.querySelectorAll('div.asset')];
        const minTop = Math.min(...allNodes.map(node => parseInt(node.style.top)));
        if (minTop < startTop) {
            const diff = Math.abs(minTop) + gap;
            allNodes.forEach(node => {
                node.style.top = `${parseInt(node.style.top) + diff}px`;
            });
        }
    }

    function findPerson(id) {
        return data.find(person => person.id === id);
    }

    function findLevel(level) {
        return levelMap.find(l => l.level === level);
    }

    function detectCollision(node) {
        return elements.find(elem => {
            const nodeLeft = parseInt(node.style.left);
            const nodeTop = parseInt(node.style.top);
            return (
                elem.top === nodeTop &&
                (elem.left === nodeLeft || (nodeLeft > elem.left && nodeLeft < elem.left + size + gap))
            );
        });
    }
}
