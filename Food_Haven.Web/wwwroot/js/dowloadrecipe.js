$('#downloadRecipeBtn').click(function () {
    let title = ($("#modalTitle").text() || "recipe").replace(/[^a-z0-9]/gi, '_').toLowerCase();
    // Lấy lại đúng txt với format không có tab đầu dòng:
    let recipeContent =
        $("#modalTitle").text() + "\n\n" +
        "Ingredients:\n" +
        parseIngredients(window.selectedRecipe.ingredients) + "\n\n" +
        "Instructions:\n" +
        parseDirections(window.selectedRecipe.directions);

    function escapeHtml(unsafe) {
        return unsafe
            .replace(/&/g, "&amp;")
            .replace(/</g, "&lt;")
            .replace(/>/g, "&gt;")
            .replace(/"/g, "&quot;")
            .replace(/'/g, "&#039;");
    }

    let html =
        `<!DOCTYPE html>
<html>
  <head>
    <meta charset="utf-8"/>
    <title>${title.replace(/_/g, ' ')}</title>
    <style>
      @import url('https://fonts.googleapis.com/css?family=Roboto:400,700&display=swap');
      body {
        font-family: 'Roboto', Arial, sans-serif;
        background-color: #e2e8f0;
        color: #4a4a4a;
        margin: 0;
        padding: 20px;
      }
      .recipe-container {
        background-color: #ffffff;
        padding: 24px 24px 20px 24px;
        border-radius: 12px;
        box-shadow: 0 4px 14px rgba(0,0,0,0.08);
        max-width: 800px;
        margin: 30px auto;
      }
      pre {
        padding: 16px 16px 12px 16px;
        border-radius: 8px;
        overflow-x: auto;
        white-space: pre-wrap;
        word-wrap: break-word;
        font-size: 1.18rem;
        background-color: #f9fafb;
        color: #222;
        line-height: 1.62;
        word-spacing: 0.1em;
        font-family: 'Fira Mono', 'Consolas', monospace;
        margin: 0;
      }
      @media (max-width: 700px) {
        .recipe-container { padding: 12px; }
        pre { font-size: 1rem; }
      }
    </style>
  </head>
  <body>
    <div class="recipe-container">
      <pre>${escapeHtml(recipeContent)}</pre>
    </div>
  </body>
</html>`;
    let blob = new Blob([html], { type: "text/html" });
    let url = URL.createObjectURL(blob);
    let link = document.createElement('a');
    link.download = title + ".html";
    link.href = url;
    document.body.appendChild(link);
    link.click();
    setTimeout(function () {
        document.body.removeChild(link);
        URL.revokeObjectURL(url);
    }, 100);
});
