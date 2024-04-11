class CategorySelector {
    constructor(categories, selectedCategories) {
        this.categories = categories;
        this.selectedCategories = selectedCategories;
    }

    //Метод для заповнення категорій
    displayCategories() {
        const selectElement = document.getElementById('categorySelect');

        this.categories.forEach(category => {
            const option = document.createElement('option');
            option.value = category.id;
            option.text = category.name;
            option.selected = this.isCategorySelected(category.id);
            selectElement.appendChild(option);
        });
    }

    // Метод для вибору відмічених категорій
    isCategorySelected(categoryId) {
        return this.selectedCategories.includes(categoryId);
    }
}