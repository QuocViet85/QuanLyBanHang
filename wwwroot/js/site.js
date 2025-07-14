// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function formatCurrency(price) {
    const vnFormatter = new Intl.NumberFormat('vi-VN', {
    style: 'currency',
    currency: 'VND',
    });
    return vnFormatter.format(price);
}

function formatCurrencyInput(idShowInput, idHideInput) {
    setTimeout(() => {
        const showInput = document.getElementById(idShowInput);
        const hideInput = document.getElementById(idHideInput);

        showInput.addEventListener('input', function(e) {
            let value = e.target.value.replace(/[^0-9]/g, '');

            hideInput.value = value;

            e.target.value = Number(value).toLocaleString('en-US');
        })
    }, 0)
    
}