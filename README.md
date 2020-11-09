## `random_xkcd::A_random_project`

> xkcd is a webcomic of romance, sarcasm, math, and language.

This project grabs a completely random comic at the click of a button.

## `A random comic for you! (Updated every few minutes)`

![A random xkcd](https://ec-xkcd.azurewebsites.net/api/xkcd)

And a dark mode one too!

![A random xkcd](https://ec-xkcd.azurewebsites.net/api/xkcddark)

## `tl;dr`

For markdown, add `![alt text](https://ec-xkcd.azurewebsites.net/api/xkcd)` to your profile page for a random comic every time you reload! (Or once every few minutes, like GitHub)

For html, add `<img src="https://ec-xkcd.azurewebsites.net/api/xkcd" alt="alt text">` to the page.

The same applies for the dark mode, where the API link is `https://ec-xkcd.azurewebsites.net/api/xkcddark`

*Please note: If this api does not work, it probably means I have ran out of azure credits!*

## `API`

Send a http request to [https://ec-xkcd.azurewebsites.net/api/xkcd](https://ec-xkcd.azurewebsites.net/api/xkcd "https://ec-xkcd.azurewebsites.net/api/xkcd") and it will respond with a random xkcd as a png.

A dark mode version is also available where a greyscale and color invert filter has been applied to the image.

[https://ec-xkcd.azurewebsites.net/api/xkcddark](https://ec-xkcd.azurewebsites.net/api/xkcddark "https://ec-xkcd.azurewebsites.net/api/xkcddark")

## `How it works`

1. On every HTTP request, the program reads the RSS feed of xkcd to obtain the latest post.
2. Then, a random post, r is selected in the range `[1, x]` where x is the latest post.
3. A HTTP request to `https://xkcd.com/<r>` is made, and the HTML is parsed.
4. The image is extracted and then loaded into memory.
5. The API then returns the extracted image.
