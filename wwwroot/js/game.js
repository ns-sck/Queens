
function validateBoardState() {
    let queens = [];

    document.querySelectorAll(".cell").forEach(cell => {
        if (cell.innerHTML.includes("♛")) {
            queens.push({
                row: parseInt(cell.getAttribute("data-row")),
                col: parseInt(cell.getAttribute("data-col"))
            });
        }
    });

    fetch("/Home/ValidateBoard", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(queens)
    })
        .then(response => response.json())
        .then(data => {
            if (data.correct) {
                alert("Congratulations! You placed all queens correctly.");
            }
        })
        .catch(error => console.error("Error:", error));
}
let timerInterval;
let timeInSeconds = 0;

function startTimer() {
    resetTimer();
    timerInterval = setInterval(function () {
        timeInSeconds++;
        const minutes = Math.floor(timeInSeconds / 60);
        const seconds = timeInSeconds % 60;

        const formattedTime = `${String(minutes).padStart(2, '0')}:${String(seconds).padStart(2, '0')}`;

        document.getElementById('timer-display').textContent = formattedTime;
    }, 1000);
}

function resetTimer() {
    clearInterval(timerInterval);
    timeInSeconds = 0;
    document.getElementById('timer-display').textContent = "00:00";
}

document.getElementById('generate-board-button').addEventListener('click', function () {
    fetch('/Home/GenerateBoard', { method: 'POST' })
        .then(response => response.json())
        .then(data => {
            if (data.success && data.board) {
                const boardContainer = document.getElementById('board-container');
                const gameBoard = document.getElementById('game-board');
                gameBoard.innerHTML = ''; 
                const colors = ["red", "green", "teal", "lime", "cyan", "orange", "pink", "brown", "purple", "yellow", "blue", "violet"];
                startTimer();
                data.board.forEach((row, rowIndex) => {
                    const rowElement = document.createElement('tr');
                    row.forEach((cell, colIndex) => {
                        const cellElement = document.createElement('td');
                        cellElement.classList.add('cell');
                        cellElement.dataset.row = rowIndex;
                        cellElement.dataset.col = colIndex;

                        const color = cell >= 100 ? colors[cell - 100] : colors[cell];
                        cellElement.style.backgroundColor = color;
                        rowElement.appendChild(cellElement);
                    });
                    gameBoard.appendChild(rowElement);
                });
            } else {
                console.error('Failed to generate board or no board data received.');
            }
        })
        .catch(error => {
            console.error('Error generating board:', error);
        });
});

document.getElementById('game-board').addEventListener('click', function (e) {
    const cell = e.target.closest('.cell');  
    if (cell) {

        const row = parseInt(cell.getAttribute('data-row'));
        const col = parseInt(cell.getAttribute('data-col'));

        fetch('/Home/PlaceQueen', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({ row: row, col: col }),
        })
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    const queenSymbol = cell.querySelector('.queen-symbol');
                    if (queenSymbol) {
                        cell.removeChild(queenSymbol);
                    } else {
                        const newQueenSymbol = document.createElement('span');
                        newQueenSymbol.classList.add('queen-symbol');
                        newQueenSymbol.textContent = '♛';
                        cell.appendChild(newQueenSymbol);
                    }
                    validateBoardState();
                }
            })
            .catch(error => console.error('Error:', error));
    }
});
