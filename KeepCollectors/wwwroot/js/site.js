// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

document.addEventListener("DOMContentLoaded", function () {

    const emblaNode = document.querySelector('.embla__viewport')
    if(!emblaNode) return;
    const embla = EmblaCarousel(emblaNode, {
        align: 'center',
        containScroll: 'trimSnaps',
        loop: true
    })

    const prevBtn = document.querySelector(".embla__prev")
    const nextBtn = document.querySelector(".embla__next")

    prevBtn.addEventListener("click", embla.scrollPrev)
    nextBtn.addEventListener("click", embla.scrollNext)

    const dotsContainer = document.querySelector(".embla__dots")

    if (!prevBtn || !nextBtn || !dotsContainer) return;

    const slides = embla.slideNodes()

    slides.forEach((_, index) => {
        const dot = document.createElement("span")
        dot.classList.add("embla__dot")

        dot.addEventListener("click", () => embla.scrollTo(index))

        dotsContainer.appendChild(dot)
    })

    const updateDots = () => {
        const dots = document.querySelectorAll(".embla__dot")
        dots.forEach(dot => dot.classList.remove("is-selected"))
        dots[embla.selectedScrollSnap()].classList.add("is-selected")

        const slides = document.querySelectorAll(".embla__slide")
        slides.forEach(slide => slide.classList.remove("is-selected"))
        slides[embla.selectedScrollSnap()].classList.add("is-selected")
    }

    embla.on("select", updateDots)
    updateDots()

})
