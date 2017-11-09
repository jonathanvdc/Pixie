files = \
	*.cs

ifeq ($(ASM_VERSION),)
	ASM_VERSION:=0.0.1.0
endif

common_options = -asm-name=Pixie -asm-version=$(ASM_VERSION) -Wall -Wextra

.PHONY: all flo dll clean
all: flo dll

flo: bin/flo/Pixie.flo
dll: bin/clr/Pixie.dll

clean:
	rm -rf bin/
	rm -rf obj/

include ../flame-make-scripts/use-ecsc.mk

bin/flo/Pixie.flo: $(files) $(libraries) | ecsc
	$(ECSC) $(files) -platform ir -runtime clr -o $@ $(common_options)

bin/clr/Pixie.dll: $(files) $(libraries) | ecsc
	$(ECSC) $(files) -platform clr -o $@ $(common_options)
