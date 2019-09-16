"use strict";
var uri = 'api/game';
var connection = null;
var clientID = 0;

$(document).ready(function () {
    getGameState();
    //connectToServer();
    // This was previously used to continuously get the state from the server incase it changed
    setInterval(function () {
        getGameState();
    }, 1000);
    $('[type="checkbox"]').click(function () {
        updateFrames();
    });
    $('#NewGameButton').click(function () {
        reset();
    });
});

//function connectToServer() {
//    const socket = new WebSocket("ws://" + document.location.hostname + ":6502");

//    socket.addEventListener('open', function (event) {
//        socket.send('Hello Server!');
//    });

//    socket.addEventListener('message', function (event) {
//        console.log("Message from server ", event.data);
//    });
//}

function reset() {
    $.ajax({
        type: "POST",
        url: uri + '/reset',
        success: updateGame
    });
}

function guess(id) {
    var isSpymaster = document.getElementById("selectSpymaster").checked;

    if (!isSpymaster) {
        $.ajax({
            type: "POST",
            url: uri + '/guess/' + id,
            success: updateGame
        });
    }
}

function getGameState() {
    $.getJSON(uri)
        .done(function (data) {
            updateGame(data);
        });
}

function updateGame(game) {
    updateFrameState(game.Frames);
    updateFrames();
    updateTurn(game);
    updateRemainingCards(game.RemainingBlueCards, game.RemainingRedCards);
}

function updateFrameState(frames) {
    for (var i = 0; i < frames.length; i++) {
        $('#f_' + i).data("state", {
            isFlipped: frames[i].IsFlipped,
            color: frames[i].Color,
            value: frames[i].Value
        });
    }
}

function updateFrames() {
    var isSpymaster = document.getElementById("selectSpymaster").checked;
    for (var i = 0; i < 25; i++) {
        var frameState = $('#f_' + i).data("state");

        // frameData may not be set yet if we haven't gotten a response from the server yet, so just hold off if
        if (frameState) {
            $('#f_' + i).attr('class', getFrameClasses(frameState, isSpymaster));
            var currentImg = $('#f_' + i).find('img').attr('src');
            if (currentImg != frameState.value) {
                $('#f_' + i).find('img').attr('src', frameState.value);
            }
        }
    }
}

function getFrameClasses(frameState, isSpymaster) {
    var classes = 'frame ';
    if (frameState.isFlipped) {
        classes += 'team-' + frameState.color;
    } else if (isSpymaster) {
        classes += 'team-' + frameState.color + '-Spymaster';
    }
    return classes;
}

function updateRemainingCards(blueCards, redCards) {
    $('.blue-remaining').html(blueCards);
    $('.red-remaining').html(redCards);
}

function updateTurn(game) {
    if (game.IsOver) {
        $('#title').html("CodeFrames - Winner " + game.Winner);
    } else {
        $('#title').html("CodeFrames - " + game.CurrentTeam + "'s turn");
    }
}