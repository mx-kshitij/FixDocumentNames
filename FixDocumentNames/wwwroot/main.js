function postMessage(message, data) {
    window.chrome.webview.postMessage({ message, data });
    window.postMessage({ message, data });
}

// Register message handler.
window.chrome.webview.addEventListener("message", handleMessage);
window.addEventListener("message", handleMessage);
// Indicate that you are ready to receive messages.
//postMessage("MessageListenerRegistered");

async function handleMessage(event) {
    const { message, data } = event.data;
    if (message === "RefreshList") {
        await refreshList();
    }
}

async function refreshList() {
    let inputText = document.getElementById("documentSearchKey")?.value;
    let documentAmount = document.getElementById("documentAmount")?.value;
    if (inputText && inputText.trim() != "") {
        let documentsResponse = await fetch(`./documents?searchKey=${inputText}&amount=${documentAmount}`);
        let documents = await documentsResponse.json();

        let documentListDiv = document.getElementById("documentList");

        let responseSpan = document.createElement("span");
        responseSpan.textContent = documents;
        documentListDiv.appendChild(responseSpan);

        let documentItems = [];
        let isDarkMode = await checkIfDarkMode();

        for (const documentItem of documents) {
            let itemDiv = document.createElement("div");

            let checkbox = document.createElement("input");
            checkbox.type = "checkbox";
            checkbox.id = `document-${documentItem.Id}`;
            checkbox.classList.add('fix-document-checkbox-item') ;

            let label = document.createElement("label");
            label.id = 'label-' + checkbox.id;
            label.htmlFor = checkbox.id;
            label.innerText = documentItem.Module + "." + documentItem.CurrentDocumentName;
            if (isDarkMode) {
                label.classList.add("dark");
            }

            itemDiv.replaceChildren(checkbox, label);

            documentItems.push(itemDiv);
        }

        documentListDiv.replaceChildren(...documentItems);
    }
    else {
        alert("Search key can not be empty!")
    }
}
async function loadTheme() {
    let isDarkMode = await checkIfDarkMode();
    if (isDarkMode) {
        document.body.classList.add("dark:bg-slate-800");
        document.getElementById("panelTitle").classList.add("dark:text-white");
        document.getElementById("documentSearchKeyLabel").classList.add("dark:text-white");
        document.getElementById("documentNewValueLabel").classList.add("dark:text-white");
        document.getElementById("documentListTitle").classList.add("dark:text-white");
        var inputs = Array.from(document.getElementsByTagName("input"));
        for (let item of inputs) {
            item.classList.add("dark");
        }
    }
}

async function checkIfDarkMode() {
    let documentsResponse = await fetch(`./theme`);
    let theme = await documentsResponse.json();
    return theme === "Dark";
}

async function fixDocumentItems() {
    let inputText = document.getElementById("documentSearchKey").value;
    let newText = document.getElementById("documentNewValue").value;

    if (inputText && newText) {
        let documentListDiv = document.getElementById("documentList");
        let checkBoxes = documentListDiv.getElementsByTagName("input");


        let checkedItems = [];
        let label = "";

        for (let item of checkBoxes) {
            if (item.checked) {
                label = document.getElementById('label-' + item.id);
                let obj = { module: label.textContent.split(".")[0], document: label.textContent.split(".")[1] };
                checkedItems.push(obj);
            }
        }

        if (checkedItems.length > 0) {
            let requestBody = { searchKey: inputText, replacementText: newText, checkedItems: checkedItems }

            //const request = new Request("./fixDocuments", {
            //    method: "POST",
            //    body: JSON.stringify(requestBody),
            //});

            //const response = await fetch(request);
            //let responseBody = await response.json();
            postMessage("FixDocuments", requestBody);
        }
        else {
            alert("At least 1 item needs to be selected!")
        }
    }
    else {
        alert("Search key and replacement value are mandatory!")
    }
}

async function clearEverything() {
    let inputText = document.getElementById("documentSearchKey");
    inputText.value = "";
    let replacementText = document.getElementById("documentNewValue");
    replacementText.value = "";
    let documentListDiv = document.getElementById("documentList");
    documentListDiv.replaceChildren();
}

async function selectAll() {
    let documentList = document.getElementsByClassName('fix-document-checkbox-item');
    let curDoc;
    for (let curDoc of documentList) {
        curDoc.checked = true;
    }
}

async function deselectedAll() {
    let documentList = document.getElementsByClassName('fix-document-checkbox-item');
    for (let curDoc of documentList) {
        curDoc.checked = false;
    }
}

async function loadListFromSearchKey() {
    const params = new URLSearchParams(window.location.search);
    const param = params.get("searchKey");
    if (param) {
        const searchInput = document.getElementById("documentSearchKey");
        searchInput.value = param;
        await refreshList();
    }
}

document.getElementById("searchButton").addEventListener("click", refreshList);
document.getElementById("clearButton").addEventListener("click", clearEverything);
document.getElementById("fixButton").addEventListener("click", fixDocumentItems);
document.getElementById("selectAllBtn").addEventListener("click", selectAll);
document.getElementById("deselectAllBtn").addEventListener("click", deselectedAll);

await loadTheme();

loadListFromSearchKey();