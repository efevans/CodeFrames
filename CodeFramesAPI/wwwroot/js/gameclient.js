"use strict";
var uri = 'api/game';
var connection;

$(document).ready(function () {
    connect();
    $('[type="checkbox"]').click(function () {
        updateFrames();
    });
    $('#NewGameButton').click(function () {
        reset();
    });
    $('#PassTurnButton').click(function () {
        passTurn();
    });
});

function connect() {
    connection = new signalR.HubConnectionBuilder()
        .withUrl("/gamehub")
        .configureLogging(signalR.LogLevel.Information)
        .build();

    connection.on("ReceiveGameUpdate", (game) => {
        console.log(game);
        updateGame(game);
    });

    connection.start().then(function () {
        console.log("connected");
        getGameState();
    });
}

function getGameState() {
    connection.invoke("SendNeedGameState").catch(err => console.error(err.toString()));
}

function guess(id) {
    var isSpymaster = document.getElementById("selectSpymaster").checked;

    if (!isSpymaster) {
        connection.invoke("SendGuess", id).catch(err => console.error(err.toString()));
        event.preventDefault();
    }
}

function passTurn() {
    connection.invoke("SendPass").catch(err => console.error(err.toString()));
    event.preventDefault();
}

function reset() {
    connection.invoke("SendNewGame").catch(err => console.error(err.toString()));
    event.preventDefault();
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