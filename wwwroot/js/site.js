
<script>
        // JavaScript to handle cell click event
    document.querySelectorAll('.cell').forEach(function (cell) {
        cell.addEventListener('click', function (e) {
            console.log("selam");
            // Get the row and column of the clicked cell
            const row = parseInt(e.currentTarget.getAttribute('data-row')); // Get data attributes from the cell itself
            const col = parseInt(e.currentTarget.getAttribute('data-col')); // Get data attributes from the cell itself

            // Send the coordinates to the server to toggle the queen (whether placing or removing)
            fetch('/Home/PlaceQueen', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({ row: row, col: col }), // Send integers
            })
                .then(response => response.json())
                .then(data => {
                    // Handle the response from the server (updated board)
                    if (data.success) {
                        // Check if the cell already contains a queen (Q)
                        if (e.target.innerHTML.trim() === "Q") {
                            // If the cell is empty, remove the queen
                            e.target.innerHTML = "";
                        } else {
                            // If the cell is empty, place a queen
                            e.target.innerHTML = "<span style='color: black; font-weight: bold;'>Q</span>";
                        }
                    }
                })
                .catch(error => console.error('Error:', error));
        });
    });
</script>

