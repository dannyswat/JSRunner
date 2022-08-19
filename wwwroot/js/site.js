﻿function createUser() {
	return new Promise(function (resolve) {
		if (!textarea('username') || !textarea('password')) { console.log('Empty data'); return; }
		$.ajax({
			url: '/Home/CreateUser?username=' + encodeURIComponent(textarea('username')) + '&password=' + encodeURIComponent(textarea('password')),
			method: 'POST'
		}).done(function () {
			textarea('username', '');
			textarea('password', '');
			resolve();
		}).fail(function (res) {
			console.log(res);
			textarea('password', '');
			resolve();
		});
	});
}

function signout() {
	return new Promise(function (resolve) {
		$.ajax({ url: '/Home/SignOut' }).fail(function (res) { resolve(); window.location = window.location });
	});
}

function loading(button, promise) {
	return new Promise(function (resolve) {
		var orig = $(button).html();
		$(button).prop('disabled', true).html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span><span class="sr-only">Loading...</span>');
		promise().then(function () { $(button).prop('disabled', false).html(orig); resolve(); });
	});
}